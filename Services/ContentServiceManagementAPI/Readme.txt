This is an ASP.NET web application, build to handle the peculiarities of VHP as against having to
tweak and retweak ANQ to fit the services and content peculiaritiess.



This service is a content service management application that 
handle perculiarities of service and content mamagement for VHP and also sends 
the service creation details to content service managemenet(ANQ) via API.

This application connects to contentservice managment database for ANQ
name : ContentServiceManagentDb. 
Note . Details that has to be posted to ANQ are handled here
being that both VHP and ANQ utilizes same Billing, UserProfile, ContentProcessing services..

It also makes connection to DndContentProcessingDb for appropraite insertion,
deletion, update and retrieval of records(peculiarities) for VHP