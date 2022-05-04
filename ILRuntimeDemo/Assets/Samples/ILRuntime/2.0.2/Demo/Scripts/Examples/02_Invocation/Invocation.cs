using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using System.Threading;

public class Invocation : ILRTBase
{
    protected override void InitializeILRuntime()
    {
        base.InitializeILRuntime();
        //这里做一些ILRuntime的注册，这个示例暂时没有需要注册的
    }

    protected override void OnHotFixLoaded()
    {
        Debug.Log("调用无参数静态方法");
        //调用无参数静态方法，appdomain.Invoke("类名", "方法名", 对象引用, 参数列表);
        appdomain.Invoke("HotFix_Project.InstanceClass", "StaticFunTest", null, null);
        //调用带参数的静态方法
        Debug.Log("调用带参数的静态方法");
        appdomain.Invoke("HotFix_Project.InstanceClass", "StaticFunTest2", null, 123);


        Debug.Log("通过IMethod调用方法");
        //预先获得IMethod，可以减低每次调用查找方法耗用的时间
        IType type = appdomain.LoadedTypes["HotFix_Project.InstanceClass"];
        //根据方法名称和参数个数获取方法
        IMethod method = type.GetMethod("StaticFunTest2", 1);

        appdomain.Invoke(method, null, 123);

        Debug.Log("通过无GC Alloc方式调用方法");
        using (var ctx = appdomain.BeginInvoke(method))
        {
            ctx.PushInteger(123);
            ctx.Invoke();
        }

        Debug.Log("指定参数类型来获得IMethod");
        IType intType = appdomain.GetType(typeof(int));
        //参数类型列表
        List<IType> paramList = new List<ILRuntime.CLR.TypeSystem.IType>();
        paramList.Add(intType);
        //根据方法名称和参数类型列表获取方法
        method = type.GetMethod("StaticFunTest2", paramList, null);
        appdomain.Invoke(method, null, 456);

        Debug.Log("实例化热更里的类");
        object obj = appdomain.Instantiate("HotFix_Project.InstanceClass", new object[] { 233 });
        //第二种方式
        object obj2 = ((ILType)type).Instantiate();

        Debug.Log("调用成员方法");
        method = type.GetMethod("get_ID", 0);
        using (var ctx = appdomain.BeginInvoke(method))
        {
            ctx.PushObject(obj);
            ctx.Invoke();
            int id = ctx.ReadInteger();
            Debug.Log("!! HotFix_Project.InstanceClass.ID = " + id);
        }

        using (var ctx = appdomain.BeginInvoke(method))
        {
            ctx.PushObject(obj2);
            ctx.Invoke();
            int id = ctx.ReadInteger();
            Debug.Log("!! HotFix_Project.InstanceClass.ID = " + id);
        }
        
        Debug.Log("调用泛型方法");
        IType stringType = appdomain.GetType(typeof(string));
        IType[] genericArguments = new IType[] { stringType };
        appdomain.InvokeGenericMethod("HotFix_Project.InstanceClass", "GenericMethod", genericArguments, null, "TestString");

        Debug.Log("获取泛型方法的IMethod");
        paramList.Clear();
        paramList.Add(intType);
        genericArguments = new IType[] { intType };
        method = type.GetMethod("GenericMethod", paramList, genericArguments);
        appdomain.Invoke(method, null, 33333);

        Debug.Log("调用带Ref/Out参数的方法");
        method = type.GetMethod("RefOutMethod", 3);
        int initialVal = 500;
        using(var ctx = appdomain.BeginInvoke(method))
        {
            //第一个ref/out参数初始值
            ctx.PushObject(null);
            //第二个ref/out参数初始值
            ctx.PushInteger(initialVal);
            //压入this
            ctx.PushObject(obj);
            //压入参数1:addition
            ctx.PushInteger(100);
            //压入参数2: lst,由于是ref/out，需要压引用，这里是引用0号位，也就是第一个PushObject的位置
            ctx.PushReference(0);
            //压入参数3,val，同ref/out
            ctx.PushReference(1);
            ctx.Invoke();
            //读取0号位的值
            List<int> lst = ctx.ReadObject<List<int>>(0);
            initialVal = ctx.ReadInteger(1);

            Debug.Log(string.Format("lst[0]={0}, initialVal={1}", lst[0], initialVal));
        }
    }

    void Update()
    {

    }
}
