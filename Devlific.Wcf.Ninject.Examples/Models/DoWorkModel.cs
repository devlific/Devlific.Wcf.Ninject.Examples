namespace Devlific.Wcf.Ninject.Examples.Models
{
    public class DoWorkModel
    {
        public string TheWork { get; set; }
    }

    public interface IDoWorkWell
    {
        string DoWork();
    }

    public class DoWorkWell : IDoWorkWell
    {
        public string DoWork()
        {
            var model = new DoWorkModel()
            {
                TheWork = "TheBusiness"
            };
            return model.TheWork;
        }
    }
}