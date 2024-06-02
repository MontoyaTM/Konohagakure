Status: 
Tag:
Links:

---
> [!note] 
>  # IDatabaseRequestData Interface

The IDatabaseRequestData Interface is used to show the required methods needed for the database to handle. Using an Interface allows for different database to be used.

``` run-csharp
public interface IDatabaseRequestData
{
	Task<bool> StoreRPRequestAsync(RequestModel request);
	Task<bool> DeleteRequestAsync(ulong RequestId);
}
```

--- 
References: