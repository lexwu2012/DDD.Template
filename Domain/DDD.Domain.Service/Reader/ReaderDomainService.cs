using System.Linq;
using DDD.Domain.Core.Model;
using DDD.Domain.Service.Reader.Dto;
using DDD.Domain.Service.Reader.Interfaces;
using DDD.Infrastructure.Domain.Repositories;
using DDD.Infrastructure.Web.Application;

namespace DDD.Domain.Service.Reader
{
    public class ReaderDomainService : DomainServiceBase, IReaderDomainService
    {
        private readonly IRepositoryWithEntity<Core.Model.Reader> _readerRepository;
        private readonly IRepositoryWithTEntityAndTPrimaryKey<Book, string> _bookRepository;
        private readonly IRepositoryWithTEntityAndTPrimaryKey<BookHistory, string> _bookHistoryRepository;

        public ReaderDomainService(IRepositoryWithEntity<Core.Model.Reader> readerRepositoryWithEntity,
            IRepositoryWithTEntityAndTPrimaryKey<Book, string> bookRepository, IRepositoryWithTEntityAndTPrimaryKey<BookHistory, string> bookHistoryRepository)
        {
            _readerRepository = readerRepositoryWithEntity;
            _bookRepository = bookRepository;
            _bookHistoryRepository = bookHistoryRepository;
        }

        public Result LendBook(int readId, string bookId, string comment)
        {
            var reader = _readerRepository.GetAll().FirstOrDefault(m => m.Id == readId);
            //todo： 先判断该用户是否还能借书或者是否有其他业务限制

            var book = _bookRepository.GetAll().FirstOrDefault(m => m.Id == bookId);
            if (null == book)
                return Result.FromError("该书不存在");

            return book.Lend(readId, comment);

            //和系统打交道，告知系统该书已经借出？
        }

        //public Result<BookHistoryDto> GetReaderLendBookHistory(int readerId)
        //{
        //    var histories = _bookHistoryRepository.GetAll().Where(m => m.ReaderId == readerId);

        //}
    }
}
