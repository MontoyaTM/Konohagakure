Status: 
Tag:
Links:

---
> [!note] 
>  # RequestModel Class

The RequestModel class is used to create an object model for rp request to be stored in the database.

``` run-csharp
public class RequestModel
{
	public ulong RequestId { get; set; }
	public ulong MemberId { get; set; }
	public string Username { get; set; }
	public ulong ServerId { get; set; }
	public string ServerName { get; set; }
	public string InGameName { get; set; }
	public string Mission {  get; set; }
	public string Attendees { get; set; }
	public string Timezone { get; set; }
}
```

---
References: