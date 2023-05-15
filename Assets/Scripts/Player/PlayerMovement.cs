using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Riptide;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;

    public float horizontalMove = 0;
    public float runSpeed = 40f;
    bool jump = false;
    Player player;
    public Animator animator;

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
        controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump);
        if(EntityMP.inSession) SendMovement();
        jump = false;
    }

    private void Start()
    {
        player = gameObject.GetComponent<EntityMP>() as Player;
    }

    private void SendMovement()
    {
        
        Message message = Message.Create(MessageSendMode.Unreliable, MessageId.PlayerMovement);
        message.AddUShort(player.id);
        message.AddVector2(transform.position);
        NetworkManager.Singleton.Client.Send(message);
    }

}
