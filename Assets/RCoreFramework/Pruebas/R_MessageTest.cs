using r_core.coresystems.optionalsystems.messages;

public class R_MessageTest : R_Message {

	public string message { get; private set; }

	public R_MessageTest()
	{
		IsLocal = true;
		SenderId = 0;
	}

	public R_MessageTest(uint _senderId, string _message): base(_senderId)
	{
		message = _message;
	}

	public void SetData(uint _senderId, string _message)
	{
		IsLocal = true;
		SenderId = _senderId;
		message = _message;
	}

}
