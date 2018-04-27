namespace BookRequest
{
    using System;

    public interface ICache  
  {
    void Add(string key, object value, TimeSpan ttl);
    object Get(string productsResource);
  }
}