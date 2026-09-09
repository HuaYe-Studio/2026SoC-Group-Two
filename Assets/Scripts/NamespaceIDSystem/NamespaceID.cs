using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct NamespaceID
{
    public string Namespace; // 物品前缀
    public string Path; // 物品路径

    public NamespaceID(string item_namespace , string path)
    {
        Namespace = item_namespace;
        Path = path;
    }

    public string FullName
    {
        get {return Namespace + ":" + Path;}
    }

    public static NamespaceID BuiltIn(string path)
    {
        return new NamespaceID("awotr" , path);
    }
}
