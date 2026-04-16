# MENTAL MODEL CHECK

## The Analogy: The Library Catalog
Imagine you are looking for a specific book in a library with 1 million volumes.

- **No Pagination**: The librarian brings you 1 million books in a giant pile and tells you to "find the one you want." (Server crash, Browser crash).
- **Pagination**: The librarian brings you a small tray with 20 books and a note: "Here are the first 20. There are 50,000 more books that match your search. Let me know when you want the next tray."
- **Filtering**: Instead of looking through all books, you tell the librarian: "Only bring me books about *C#* written *after 2020*." The librarian does the filtering in the archive (SQL Server) and only brings the relevant books to the desk.
- **Sorting**: You ask for the books to be sorted by "Author Name" alphabetically. Again, the librarian does the sorting in the archive before bringing them to you.
- **API Versioning**: The library changes its filing system from "Dewey Decimal" to a "New System." To avoid confusing regular visitors, they keep the old catalog (v1) and the new catalog (v2) running side-by-side for a year.

## Where the Analogy Breaks Down
In a library, the librarian is a human who can "guess" where a book is. In an API, if you don't explicitly call `.OrderBy()` before `.Skip()`, the database does not guarantee the order of the rows. This means a user might see the same record on Page 1 and Page 2—a phenomenon called **Pagination Drift**.
