using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector2 ratio = new Vector2(16, 9);
    public Transform target;
    public Camera thisCamera;
    public Transform playerFollowBorderRight;
    public Transform playerFollowBorderLeft;
    public Transform playerStopBorderRight;
    public Transform playerStopBorderLeft;
    public float cameraStopLeft;
    public float cameraStopRight;
    public GameObject dungeonObj;
    public GameObject background;

    RectTransform bgImage;
    Dungeon dungeon;

    void Awake()
    {
        dungeon = dungeonObj.GetComponent<Dungeon>();

        playerStopBorderLeft.GetComponent<BoxCollider2D>().size = new Vector2(0.3f, thisCamera.orthographicSize * 4);
        playerStopBorderRight.GetComponent<BoxCollider2D>().size = new Vector2(0.3f, thisCamera.orthographicSize * 4);

        bgImage = background.GetComponent<RectTransform>();
    }

    void LateUpdate()
    {
        try
        {
            cameraStopRight = dungeon.GetRoomById(dungeon.currentRoomId).RoomData.size.x * dungeon.scale;
            playerStopBorderLeft.position = transform.position - new Vector3(thisCamera.orthographicSize * ratio.x / ratio.y, 0, 0);
            playerStopBorderRight.position = transform.position + new Vector3(thisCamera.orthographicSize * ratio.x / ratio.y, 0, 0);
            float cameraLerp = Mathf.InverseLerp(cameraStopLeft, cameraStopRight, transform.position.x);
            background.transform.position = thisCamera.transform.position + new Vector3(-Mathf.Lerp(-bgImage.rect.width / 500, bgImage.rect.width / 500, cameraLerp), 0, 100);

        } catch
        {
            cameraStopRight = thisCamera.orthographicSize * ratio.x / ratio.y * 2;
        }
        if (target.position.x >= cameraStopLeft + thisCamera.orthographicSize * ratio.x / ratio.y &&
            target.position.x <= cameraStopRight - thisCamera.orthographicSize * ratio.x / ratio.y)
        {
            if (target.position.x <= playerFollowBorderLeft.position.x)
                transform.position = new Vector3(
                    target.position.x + Mathf.Abs(transform.position.x - playerFollowBorderLeft.position.x),
                    transform.position.y, 
                    transform.position.z);
            else if (target.position.x >= playerFollowBorderRight.position.x)
                transform.position = new Vector3(
                    target.position.x - Mathf.Abs(transform.position.x - playerFollowBorderRight.position.x), 
                    transform.position.y, 
                    transform.position.z);
        }

    }

    public void warpToTarget()
    {
        transform.position = new Vector3(
            Mathf.Min(
                Mathf.Max(
                    target.transform.position.x, cameraStopLeft + thisCamera.orthographicSize * ratio.x / ratio.y
                    ),
                cameraStopRight - thisCamera.orthographicSize * ratio.x / ratio.y
                ),
            transform.position.y,
            transform.position.z);
    }

}
