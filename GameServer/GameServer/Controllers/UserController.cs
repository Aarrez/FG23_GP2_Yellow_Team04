using Microsoft.AspNetCore.Mvc;
using SharedLibrary;

namespace GameServer.Controllers;
[ApiController]
[Route("[controller]")]
public class UserController<TStruct, TEnum> : ControllerBase
    where TStruct : struct
    where TEnum : Enum 
{
    [HttpGet]
    public UserData<TStruct, TEnum> Get
        ([FromQuery] UserData<TStruct, TEnum> userData)
    {
        var user = new UserData<TStruct, TEnum>();

        return user;
    }

    [HttpPost]
    public void Post(UserData<TStruct, TEnum> userData)
    {
        Console.WriteLine("Data has been added to database");
    }
}
