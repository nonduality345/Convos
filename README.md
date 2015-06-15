# Convos

An API to serve short conversations

For detailed information, please consult the wiki for this repo.

Just a note on assumptions:

Every Convo only occurs between two users and only two users. For possible expansion, this could be opened up to have conversations among multiple users. Please see the section on possible improvements.

Deleting a Convo deletes the Convo for both users in the conversation. Please see the section on possible improvements for more information on how I would change this.

For authorization, in the future, an authorization framework like OAuth would be utilized to identify the calling client. For the purposes of this small example I am only passing in the user Id as a field in the header named X-Authorization. In a real world application, I would use an OAuth service to obtain an access token for the resources the user has access to and place it in this header, but for illustrative purposes, this header is only containing the user's id.

To edit a Convo or Message, I avoided the use of PUT and instead opted for PATCH. The reason for this is that the entire object does not need to be updated and I'm explicitly disallowing fields to be updated. Partial updates allow for more control over the data, which is why I chose to PATCH objects instead of PUTing them.
