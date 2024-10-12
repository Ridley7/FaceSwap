[System.Serializable]
public class ResponseTileImage
{
	public TaskInfo task;
    public string image_file;
    public string image_type;
}

[System.Serializable]
public class TaskInfo
{
    public string task_id;
    public string status;
}

/*
{
    "task": {
        "task_id": "cabb8ef0-aac8-4443-84b1-cb663994e920",
        "status": "TASK_STATUS_SUCCEED"
    },
    "image_file": "iVBORw0KGgoAAAANSUhEUgAABAAAAAQACAYAAAB/HSuDAAAgAElEQVR4AbzB66",
    "image_type": "png"
}

*/