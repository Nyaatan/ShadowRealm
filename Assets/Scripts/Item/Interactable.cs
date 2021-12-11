using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public KeyCode interactKey = KeyCode.E;
    public string popUpText;
    public bool interactable = true;
    Collider2D collision;
    bool triggerStay = false;

    // Start is called before the first frame update
    public virtual void Start()
    {
        popUpText = string.Format("Press {0}", interactKey.ToString());
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (Input.GetKeyDown(interactKey) && collision != null) if(collision.gameObject.name == "Player" && triggerStay)
        {

                //StartCoroutine(DisableColliderForSeconds(3f, GetComponent<Collider2D>()));
                Interact(collision.gameObject);

        }
    }

    public virtual void Interact(GameObject obj) { }

    public virtual void showPopUp()
    {
        Tooltip.Instance.ShowTooltip(popUpText);
    }

    public virtual void hidePopUp()
    {
        Tooltip.Instance.HideTooltip();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        this.collision = collision;
        if (collision.gameObject.name == "Player")
        {
            showPopUp();
            triggerStay = true;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        this.collision = null;
        if (collision.gameObject.name == "Player")
        {
            hidePopUp();
            triggerStay = false;
        }
    }

    public IEnumerator DisableColliderForSeconds(float time, Collider2D collider)
    {
        collider.enabled = false;
        yield return new WaitForSeconds(time);
        collider.enabled = true;
    }
}
