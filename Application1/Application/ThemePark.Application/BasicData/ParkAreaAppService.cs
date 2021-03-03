using Abp.AutoMapper;
using AutoMapper.QueryableExtensions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ThemePark.Application.BasicData.Interfaces;
using ThemePark.ApplicationDto.BasicData;
using ThemePark.Core.BasicData;
using ThemePark.Core.BasicData.Repositories;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.EntityFramework;

namespace ThemePark.Application.BasicData
{
    /// <summary>
    /// 公园分区，实现数据权限
    /// </summary>
    /// <seealso cref="ThemePark.Application.ThemeParkAppServiceBase"/>
    /// <seealso cref="ThemePark.Application.BasicData.Interfaces.IParkAreaAppService"/>
    public class ParkAreaAppService : ThemeParkAppServiceBase, IParkAreaAppService
    {
        #region Fields

        private readonly IParkAreaDomainService _parkAreaDomainService;
        private readonly IParkAreaRepository _parkAreaRepository;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ParkAreaAppService"/> class.
        /// </summary>
        /// <param name="parkAreaRepository">The park area repository.</param>
        /// <param name="parkAreaDomainService">The park area domain service.</param>
        public ParkAreaAppService(IParkAreaRepository parkAreaRepository, IParkAreaDomainService parkAreaDomainService)
        {
            _parkAreaRepository = parkAreaRepository;
            _parkAreaDomainService = parkAreaDomainService;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// 新增公园分区
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Result.</returns>
        public async Task<Result<ParkArea>> AddParkAreaAsync(AddParkAreaInput input)
        {
            var reuslt = await _parkAreaDomainService.AddParkAreaAsync(input.MapTo<ParkArea>());

            return reuslt;
        }

        /// <summary>
        /// 根据条件获取公园分区
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<TDto> GetParkAreaAsync<TDto>(IQuery<ParkArea> query)
        {
            return await _parkAreaRepository.AsNoTracking().FirstOrDefaultAsync<ParkArea, TDto>(query);
        }

        /// <summary>
        /// 根据条件获取公园分区
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<List<TDto>> GetParkAreasAsync<TDto>(IQuery<ParkArea> query)
        {
            return await _parkAreaRepository.AsNoTracking().ToListAsync<ParkArea, TDto>(query);
        }

        /// <summary>
        /// 获取分区所关联的公园
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;DropdownDto&gt;.</returns>
        public async Task<DropdownDto> GetParkAreaOwnParksAsync(int id)
        {
            var query = _parkAreaRepository.GetParkNodesQuery(id).Where(o => o.ParkId.HasValue);

            return await query.Select(o => new DropdownItem()
            {
                Text = o.Park.ParkName,
                Value = o.ParkId.Value
            }).ToDropdownDtoAsync();
        }

        /// <summary>
        /// 获取所有公园分区，树形结构
        /// </summary>
        public async Task<List<ParkAreaTreeDto>> GetParkAreaTreeAsync()
        {
            var areas = await _parkAreaRepository.GetAll().ProjectTo<ParkAreaTreeDto>().ToListAsync();

            //create tree structure
            //by group way
            var group = areas.OrderBy(o => o.Id).GroupBy(o => o.ParentPath).OrderBy(o => o.Key).ToList();
            var roots = group.Where(o => o.Key.Equals(ParkArea.TopPath)).SelectMany(o => o).ToList();
            var nodes = group.Where(o => !o.Key.Equals(ParkArea.TopPath)).ToList();
            foreach (var item in nodes)
            {
                var parent = GetParent(roots, item.First().ParentPath);
                parent.Children = item.ToList();
            }

            //by recursive way
            //var roots = areas.Where(o => o.Level == ParkArea.TopLevel).ToList();
            //foreach (var root in roots)
            //{
            //    GetChildrenFromAreas(root, areas);
            //}

            return roots;
        }

        /// <summary>
        /// 更新公园分区树形结构
        /// </summary>
        /// <param name="input">确保各节点以DLR顺序，或者是层级顺序</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        public async Task<Result> UpdateParkAreaTreeAsync(UpdateParkAreaTreeInput input)
        {
            //check?

            var allAreaIds = input.Select(o => o.Id).ToArray();

            //get all park areas
            var areas = await _parkAreaRepository.GetAll().Where(o => allAreaIds.Contains(o.Id)).ToListAsync();
            //order by input order
            areas = areas.OrderBy(o => Array.IndexOf(allAreaIds, o.Id)).ToList();

            //adjust the parent-child relationship
            foreach (var area in areas)
            {
                input.First(o => o.Id == area.Id).MapTo(area);
            }

            //update every node
            var result = await _parkAreaDomainService.UpdateParkAreaTreeAsync(areas);

            return result;
        }

        /// <summary>
        /// 将所有孩子节点构造成树，递归
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="areas">The all children areas.</param>
        private void GetChildrenFromAreas(ParkAreaTreeDto parent, IList<ParkAreaTreeDto> areas)
        {
            //get all children
            var startStr = parent.ParentPath + ParkArea.Separator + parent.Id;
            areas = areas.Where(o => o.ParentPath.StartsWith(startStr)).OrderBy(o => o.ParentPath).ThenBy(o => o.Id).ToList();

            List<ParkAreaTreeDto> sons = areas.Where(o => o.Level == parent.Level + 1).ToList();
            parent.Children = sons;
            foreach (var area in sons)
            {
                GetChildrenFromAreas(area, areas.Except(sons).ToList());
            }
        }

        /// <summary>
        /// 根据节点路径找到父节点
        /// </summary>
        /// <param name="roots">The roots.</param>
        /// <param name="path">The path.</param>
        /// <returns>ParkAreaTreeDto.</returns>
        /// <exception cref="System.Exception">the root is not a top level.</exception>
        private ParkAreaTreeDto GetParent(IList<ParkAreaTreeDto> roots, string path)
        {
            var array = path.Split(ParkArea.Separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse).ToList();

            var result = roots.First(o => o.Id == array[1]);
            if (result.Level != ParkArea.TopLevel)
            {
                throw new Exception("the root is not a top level.");
            }

            for (int i = 2; i < array.Count(); i++)
            {
                result = result.Children.First(o => o.Id == array[i]);
            }

            return result;
        }

        #endregion Methods
    }
}