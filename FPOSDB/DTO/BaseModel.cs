using FPOSDB.Attributes;

namespace FPOSDB.DTO
{
    public interface IModel
    {
        string DisplayName { get; }
        string PrimaryKey { get; }
    }

    public abstract class BaseModel<T> : IModel
    {
        [NotSerializable]
        public abstract string DisplayName { get; }
        [NotSerializable]
        public abstract string PrimaryKey { get; }
        public bool AreEqual(T item)
        {
            bool areEqual = true;
            var props = this.GetType().GetProperties();

            foreach (var prop in props)
            {
                var thisPropValue = (int)prop.GetValue(this, null);
                var otherPropValue = (int)prop.GetValue(item, null);
                if (thisPropValue != otherPropValue)
                {
                    areEqual = false;
                    break;
                }
            }

            return areEqual;
        }

        public abstract override string ToString();
    }
}