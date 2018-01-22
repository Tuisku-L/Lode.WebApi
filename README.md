##Lode
A RESTful framework of ASP.Net Core.

# Features
* Support ASP.Net Core
* Qucik to get started (minimal architecture)
    - Install nuget package
    - Create conteoller
    - Done
* Plugin system (plugin architecture)
    - One project one plugin
        - Configure by json

##How to use
* Install Lode's nuget package.
```
PM> Install-Package Lode
```

* Create a class that implement 'IController'
```
public class TestController : IController
{
    //blablabla
}
```

* Add method that use attribute 'Route' and return 'IActionResult'
```
public class TestController : IController
{
    [Route("/Test")]
    public IActionResult Hello(){
        return new JsonResult(obj: new { Content = " Hello World! " });
    }
}
```

* Run !