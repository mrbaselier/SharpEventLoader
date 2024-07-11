# SharpEventLoader
Port from SharpEventPersist but with a dynamic SharpEventLoader (based on CLI arguments).

This is the SharpEventLoader from Improsec's SharpEventPersist (https://github.com/improsec/SharpEventPersist). The original does include a dynamic way of importing shellcode into different eventlogs with different source's and ID's. However, using SharpEventLoader the loading is static from the "Key Management Service" with the source set to "Persistence" and the InstanceID set to 1337. We can hard-change this in the C# code or use EventLogForRedTeams and also change this in the C# code. I wanted to create the SharpEventLoader so it dynamically takes these values from CLI arguments (just like SharpEventPersist does). And so... I did make a few changes to make this happen. 

I know my code could be better... but for now it does the job. Anyone willing to improve this code please do and share the changes with me. 

Thank you!
