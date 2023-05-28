using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Riptide;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;

    public float horizontalMove = 0;
    public float runSpeed = 40f;
    public bool jump = false;
    Player player;
    public Animator animator;

    public ushort authority = 1;

    // Update is called once per frame
    void Update()
    {
        Vector3 mouseScreen = Input.mousePosition;
        Vector3 mouse = Camera.main.ScreenToWorldPoint(mouseScreen);
        transform.localScale = new Vector3(mouse.x > transform.position.x ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        if (Input.GetButtonDown("Jump")) jump = true;
    }

    void FixedUpdate()
    {
        if (!EntityMP.inSession || (player.id == authority && (Player.List[authority] as Player).isLocal))
        {
            controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump);
        }
        if (EntityMP.inSession && player.isLocal)
        {
            if (player.id != authority)
            {
                controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump);
                SendMovement();
            }
            else SendPosition(horizontalMove == 0 ? 0 : -1);
        }
        jump = false;
    }

    public void ForceMove(long timestamp)
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump);
        SendPosition(timestamp);
    }

    private void Start()
    {
        player = gameObject.GetComponent<EntityMP>() as Player;
    }

    private void SendMovement()
    {
        long ts = ResearchManager.GetTimestamp();
        player.AddToHistory(transform.position, ts);
        Message message = Message.Create(MessageSendMode.Unreliable, MessageId.PlayerInput);
        message.AddFloat(horizontalMove);
        message.AddBool(jump);
        message.AddLong(ts);
        NetworkManager.Singleton.Client.Send(message);
    }

    private void SendPosition(long timestamp)
    {
        Message message = Message.Create(MessageSendMode.Unreliable, MessageId.PlayerMovement);
        message.AddUShort(player.id);
        message.AddVector2(player.transform.position);
        message.AddFloat(GetComponent<Rigidbody2D>().velocity.y);
        message.AddLong(timestamp);
        foreach(Player p in Player.List.Values) if(p.id != authority)
            NetworkManager.Singleton.Server.Send(message, p.id);
    }

}
