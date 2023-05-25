using System;

namespace Core
{
    public static class Hash
    {
        public static Guid Create()
        {
            return Guid.NewGuid();
        }
    }
}