
# HSZF_NHF - NLB Bus Schedule Management

## Overview

This project is developed as part of the Advanced Software Development course for the 2024/25 semester. The goal of this project is to create a layered application to process and manage bus schedule data for the NLB bus company. The application reads data from a JSON file, saves it into a database, and provides functionalities to manipulate and retrieve the data.

## Assignment Details

### NLB
The NLB bus company hired you to help process their data to prove that their routes are punctual.

### Task Requirements
1. **Data Loading**: Load a JSON file containing regions and bus routes. Save data into a database.
2. **Data Handling**: Add new routes if the region already exists.
3. **Region Management**: Create, modify, and delete regions during runtime.
4. **Manual Route Addition**: Manually add new routes and notify users if a route has the least delay.
5. **Statistics Generation**: Generate and save statistics including:
   - Buses with delays less than 10 minutes per region.
   - Average delays, least and most delayed routes per region.
   - Destination with the most delays per region (delays of 5 minutes or less are not considered delays).
6. **Output Path**: Optionally specify a path to save the output file.
7. **Region Listing and Searching**: List regions and provide search/filter functionality.

## Project Structure
The project is structured into several layers and projects:
- **Application**: Business logic and services.
- **Domain**: Models and entities.
- **Infrastructure**: Database context and persistence logic.
- **Presentation**: Console application.
- **Tests**: Unit tests for business logic.

## Code Specifics
- **Data Loading**: Implemented in `DataLoader` class which reads JSON and saves to the database.
- **Data Handling**: Managed by `BusService` class which updates existing regions with new routes.
- **Region Management**: Handled by `RegionManager` class which provides CRUD operations for regions.
- **Manual Route Addition**: Implemented in `RouteManager` class which allows adding routes and notifies users of the least delay.
- **Statistics Generation**: `StatisticsGenerator` class generates and saves required statistics to a file.
- **Region Listing and Searching**: Provided by `RegionSearch` class which lists and filters regions based on user input.

## Conclusion
This project demonstrates the ability to develop a layered application with a focus on data processing, management, and user interaction. The code is structured to ensure maintainability and scalability.
````
