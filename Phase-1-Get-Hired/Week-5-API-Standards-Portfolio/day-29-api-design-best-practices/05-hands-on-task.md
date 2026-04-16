## Task

Implement a professional, paginated, and filterable "Appointments List" endpoint for the Clinic API.

## Files to Create / Modify

1. `DTOs/PagedResult.cs` — Create the generic `PagedResult<T>` envelope.
2. `DTOs/AppointmentResponse.cs` — Create the response DTO.
3. `Controllers/AppointmentController.cs` — Implement the `GET /api/appointments` endpoint.

## Expected Output / Behavior

1. **Pagination**: The endpoint must accept `page` and `pageSize` query parameters.
2. **Filtering**: The endpoint must accept a `status` filter (e.g., `Confirmed`, `Pending`, `Cancelled`).
3. **Sorting**: The endpoint must accept a `sortBy` parameter (e.g., `Date` or `PatientName`).
4. **Envelope**: The response must be a `PagedResult<AppointmentResponse>` containing the data and the total count.
5. **Verification**: 
   - Call `GET /api/appointments?page=1&pageSize=5`. Verify you get exactly 5 items.
   - Call `GET /api/appointments?status=Confirmed`. Verify only confirmed appointments are returned.
   - Verify that the total count in the envelope matches the total number of filtered items in the database.

## Do NOT Do This

- **Do NOT return a raw `List<T>`**.
- **Do NOT call `.ToList()` before applying `.Where()`, `.OrderBy()`, `.Skip()`, or `.Take()`**.
- **Do NOT use hardcoded page sizes** (always allow the client to specify it, but set a maximum limit like 100 to prevent DOS attacks).

## Self-Review Checklist

- [ ] Is the total count calculated *before* the `Skip/Take` is applied?
- [ ] Are all filters applied to the `IQueryable`?
- [ ] Does the `PagedResult` include `TotalPages` as a calculated property?
- [ ] Is the sorting logic implemented using `OrderBy` or `OrderByDescending`?
