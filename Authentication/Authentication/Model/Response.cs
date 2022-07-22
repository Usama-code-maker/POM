namespace Authentication.Model
{
    public class Response
    {
        public int Id { get; set; }
        public string StatusCode { get; set; } 
        public string Message { get; set; }       
        public bool status { get; set; }
        public object Token { get; internal set; }
    }
}
