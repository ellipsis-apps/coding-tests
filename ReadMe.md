## Summary
### This app is to complete a coding test for a job application. The specifications for the application were provided by the client. The specifications are documented below.

Your task is to build an application that supports the requirements outlined below. Outside of the requirements outlined, as well as any language limitations speciﬁed by the technical implementation notes below and/or by the hiring manager(s), the application is your own design from a technical perspective.
This is your opportunity to show us what you know! Have fun, explore new ideas, and as noted in the Questions section below, please let us know if you have any questions regarding the requirements!
## Requirements
### Requirement #1: Store a Purchase Transaction
Your application must be able to accept and store (i.e., persist) a purchase transaction with a description, transaction date, and a purchase amount in United States dollars. When the transaction is stored, it will be assigned a unique identiﬁer.
#### Field requirements
- Description: must not exceed 50 characters
- Transaction date: must be a valid date format
- Purchase amount: must be a valid positive amount rounded to the nearest cent
- Unique identiﬁer: must uniquely identify the purchase
### Requirement #2: Retrieve a Purchase Transaction in a Speciﬁed Country’s Currency
Based upon purchase transactions previously submitted and stored, your application must provide a way to retrieve the stored purchase transactions converted to currencies supported by the Treasury Reporting Rates of Exchange API based upon the exchange rate active for the date of the purchase.
https://ﬁscaldata.treasury.gov/datasets/treasury-reporting-rates-exchange/treasury-reporting-rates-of-exchange
The retrieved purchase should include the identiﬁer, the description, the transaction date, the original US dollar purchase amount, the exchange rate used, and the converted amount based upon the speciﬁed currency’s exchange rate for the date of the purchase.
#### Currency conversion requirements
- When converting between currencies, you do not need an exact date match, but must use a currency conversion rate less than or equal to the purchase date from within the last 6 months.
- If no currency conversion rate is available within 6 months equal to or before the purchase date, an error should be returned stating the purchase cannot be converted to the target currency.
- The converted purchase amount to the target currency should be rounded to two decimal places (i.e., cent).
### Technical Implementation
The technical implementation, including frameworks, libraries, etc. is your own design except for the language. The solution should be implemented in either C# or Java. If you want to use a JVM-based language other than Java, please gain permission to do so in advance of implementing the solution.
You should build this application as if you are building an application to be deployed in a Production environment. This should be interpreted to mean that all functional automated testing you would include for a Production application should be expected. Please note that non-functional test (e.g., performance testing) automation is not needed.

## Notes
The folder "MudBlazorVersion" has the code submitted for the test. It uses the MudBlazor set of UI componentes.

After the testing organization has cloned my repo, I intend to re-write the app (in different folders) using various Blazor UI libraries, as well as re-writing it in React & Angular.

The Dockerfile may not be correct. My personal device has an ARM-64 chip running Windows Home 11. At the time of submission, Docker had a pre-release version of Docker Desktop.
