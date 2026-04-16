# MENTAL MODEL CHECK

## The Analogy: The Restaurant Menu
Think of the relationship between an Entity and a DTO like a **Kitchen** vs. a **Menu**.

- **The Entity is the Kitchen**: It contains everything—the raw ingredients, the trash cans, the industrial cleaners, and the messy prep tables. You never let a customer walk into the kitchen.
- **The DTO is the Menu**: It is a carefully curated, polished version of what the kitchen can provide. It shows the "Dish Name" and "Price," but it doesn't show the "Wholesale Cost of Cabbage" or the "Kitchen Staff's Shift Schedule."

## Where the analogy breaks down
In a restaurant, the menu is static. In an API, the "Menu" (DTO) can change dynamically based on who is asking. An `Admin` might get a `ProductAdminResponseDto` (showing profit margins), while a `Customer` gets a `ProductResponseDto` (showing only the retail price).
