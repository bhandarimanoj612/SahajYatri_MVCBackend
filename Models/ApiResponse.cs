using System.Net;

namespace Sahaj_Yatri.Models
{
    public class ApiResponse
    {
        //this file is mode to control https respose 
        public ApiResponse() 
        { 

            ErrorMessages = new List<string>();
        }

        //status code 200 ok 
        public HttpStatusCode StatusCode { get; set; }
        //if the request is success
        public bool IsSuccess { get; set; } = true;
        //if there is error we display error
        public List <string >ErrorMessages { get; set; }

        //list of  items
        public object Result { get; set; }
       

        
    }
}
