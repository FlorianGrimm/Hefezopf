namespace Hefezopf.Fundament.Schema
{
    public class HZDBUserDefinedType : HZDBSchemaOwned
    {
        public HZDBUserDefinedType()
        {
        }

        public System.Data.SqlDbType Type { get; set; }

        public int Size { get; set; }
    }
}