using System.Linq;
using Microsoft.OData.Client;

/*
 * 使用例子
 * 先从ODataV4ContextBase派生出一个类
    class ODataContainer : ODataV4ContextBase
    {
        public ODataContainer(string serviceRoot):base(serviceRoot)
        {
           
        }

        public IQueryable<Person> People
        {
            get
            {
                return base.CreateNewQuery<Person>("People");
            } 
                
        }
    }

* 调用方法如下：
    string serviceUri = "http://localhost:20491/";
    var container = new ODataContainer(serviceUri);
    var query = container.People.Where(p => p.Description!=null);
*/

namespace ThemePark.Common
{
    /// <summary>
    /// OData V4 Version ASP.NET WebAPI OData RestFull Client Context
    /// <remarks>v1.0 2015.4.1 http://www.pwmis.com/sqlmap </remarks>
    /// </summary>
    public class ODataV4ContextBase : DataServiceContext
    {
        /// <summary>
        /// V4 OData Init
        /// </summary>
        /// <param name="serviceRoot">V4 OData ASP.NET WebAPI url base</param>
        public ODataV4ContextBase(string  serviceRoot)
            : base(new System.Uri( serviceRoot), ODataProtocolVersion.V4)
        {
            if (!serviceRoot.EndsWith("/"))
                serviceRoot = serviceRoot + "/";
            GeneratedEdmModel gem = new GeneratedEdmModel(serviceRoot);
            this.Format.LoadServiceModel = gem.GetEdmModel;
            this.Format.UseJson();
        }

        /// <summary>
        /// Create a New OData Service Query
        /// </summary>
        /// <typeparam name="T">entity Type class</typeparam>
        /// <param name="name">entitySetName</param>
        /// <returns></returns>
        public IQueryable<T> CreateNewQuery<T>(string name) where T:class
        {
            return base.CreateQuery<T>(name);
        }

        class GeneratedEdmModel
        {
            private string ServiceRootUrl;
            public GeneratedEdmModel(string serviceRootUrl)
            {
                this.ServiceRootUrl = serviceRootUrl;
            }

            public Microsoft.OData.Edm.IEdmModel GetEdmModel()
            {
                string metadataUrl = ServiceRootUrl + "$metadata";
                return LoadModelFromUrl(metadataUrl);
            }

            private Microsoft.OData.Edm.IEdmModel LoadModelFromUrl(string metadataUrl)
            {
                System.Xml.XmlReader reader = CreateXmlReaderFromUrl(metadataUrl);
                try
                {
                    return Microsoft.OData.Edm.Csdl.EdmxReader.Parse(reader);
                }
                finally
                {
                    ((System.IDisposable)(reader)).Dispose();
                }
            }

            private static System.Xml.XmlReader CreateXmlReaderFromUrl(string inputUri)
            {
                return System.Xml.XmlReader.Create(inputUri);
            }
        }
    }
}
