## How you ran the app

1) After acquiring the assessment, I ensured that I have all the necessary prerequisites installed such as the correct .NET version, Node.js, and Angular. After that
I did a 'dotnet restore' and 'npm install' to install any dependencies
2) Once it is both done, dotnet run --project src/LabOpsDesk.Api to startup the App for both frontend and backend
3) Open http://localhost:5186/ in browser


## One design trade-off you made
1) Splitting UI error state into modal level and page level to separate the workflow

## AI Assistance & Review 
-- How you used AI assistance
1) Generation of Frontend table and modal UI.
2) Generation of validation logics and TypeScript interfaces
3) Generation of code to handle read/write in CSV and JSON

-- How you reviewed the result
1) Manually ensured that validation and HTTP error responses (400, 409) is displayed inside the modal, while successful requests will properly close the modal and display success message on page-level.
2) Ensure that the unit tests pass successfully without errors
3) Tested the functions of the UI against the requirements and ensured that the end result matches the expected result.

## What you would add with more time
-- Build a GitHub Actions/Azure DevOps pipeline:
1) Automating build verification for both .NET and Angular projects
2) Executing backend and frontend unit test upon pull requests
3) Handle website deployment

-- Authentication & Authorization
1) Enable .NET Identity or OAuth for lab users
2) Implement Role-Based Access Control to handle actions for each lab user. (Eg: Lab Technician can only view inventory, adjust stock quantities)

-- UI Features
1) Enable search function, pagination and filters to handle larger volumes of assets/parts

-- Backend features
Audit Logging: Maintain a historical log for checkout transitions and stock quantity adjustments for regulatory and compliance tracking.
1) Implement audit logging to track history of items (DateTime, User)