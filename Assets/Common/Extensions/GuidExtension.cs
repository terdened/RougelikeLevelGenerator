using System;

public static class GuidExtension
{
    public static string ToId(this Guid guid)
    {
        return guid.ToString().Split('-')[0];
    }
}