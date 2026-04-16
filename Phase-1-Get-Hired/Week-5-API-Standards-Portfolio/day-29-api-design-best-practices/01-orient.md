# ORIENT

## Why This Topic Exists
A "working" API is not a "professional" API. If your endpoint returns 10,000 records in a single JSON array, you've created a performance bomb. If your client has to download the entire list just to find one doctor, you've wasted bandwidth and memory.

Professional APIs use **Pagination**, **Filtering**, and **Sorting** to allow the client to request exactly what they need and nothing more. This transforms a "dump of data" into a "queryable resource."

## Android/Kotlin Parallels
In Android, you've likely used the **Paging Library (Paging 3)**. Paging 3 is designed specifically to handle the `PagedResult` envelope we will build today.
- **Backend**: Sends a `PagedResult` (data + metadata like total count).
- **Frontend**: Uses that metadata to determine if there is a "Next Page" and triggers the next API call as the user scrolls.
- **Filtering**: Instead of using `filter { ... }` in Kotlin (which filters in RAM), you send query parameters to the API so the database does the filtering.

## The Bigger Picture
The goal is to maintain the **IQueryable** chain. Any filtering, sorting, or pagination must happen **at the database level** before the data is transferred over the network. If you call `.ToListAsync()` first and then `.Where()`, you've committed the "Performance Cliff" error from Day 18.
