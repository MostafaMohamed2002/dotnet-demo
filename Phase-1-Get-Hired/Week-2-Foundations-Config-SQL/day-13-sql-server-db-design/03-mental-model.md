# MENTAL MODEL CHECK

## The Analogy: The Filing Cabinet
Imagine your database as a massive filing cabinet.

- **Tables**: Different drawers (one for Patients, one for Appointments).
- **Primary Key**: The unique label on the folder.
- **Foreign Key**: A note inside the Patient folder saying "See Folder #42 in the Appointments drawer."
- **Clustered Index**: The folders are physically sorted by the label (1, 2, 3...).
- **Non-Clustered Index**: A small notebook on top of the cabinet that says: "All Cardiology doctors are in folders 5, 12, and 88."

## Where the analogy breaks down
In a filing cabinet, finding a folder by a non-indexed attribute (like "Patients over 60") requires you to open every single folder and check the age. In a database, if you don't have an index, SQL does exactly that (a Table Scan). However, unlike a human, SQL can perform "Parallel Scans" and use "Execution Plans" to optimize the search in ways a person cannot.
