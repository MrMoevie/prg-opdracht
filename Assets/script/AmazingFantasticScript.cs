using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.UI;

public class AmazingFantasticScript : MonoBehaviour
{
    public float speed = 1f;
    public float accel = 1f;
    public float jumpheight = 1f;
    private Rigidbody2D rb;
    public Animator animator;
    public SpriteRenderer spriteR;
    public float attacking;
    public int swords;
    public int health;
    public float stealth;
    public int [] inventoryArray;
    public GameObject[] slots;
    public GameObject item;
    public Sprite[] itemsprites;
    public GameObject inventory;
    void OnEnable()
    {
        rb = GetComponent<Rigidbody2D> ();
    }
    //LINK STARRTO
    void Start()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        inventoryArray = new int[slots.Length];
    }

    // Update is called once per frame
    void Update()
    {
        float scale = Screen.width / 2f;
        inventory.transform.localScale = new Vector3(scale,scale,1);
        if (Input.GetKeyDown("e")) inventory.SetActive(!inventory.activeSelf);
        bool sneaking = Input.GetKey("left shift");
        transform.localScale = sneaking ? new Vector3(2.2f, 0.35f, 5) : new Vector3(1.1f, 1, 1);
        if (sneaking == true)
        {
            rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, speed * Input.GetAxis("Horizontal"), accel * Time.deltaTime), rb.velocity.y - 24f * Time.deltaTime);
        }
        else
        {
            rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, speed * Input.GetAxis("Horizontal"), accel * Time.deltaTime), rb.velocity.y - 8f * Time.deltaTime);
        }

        animator.SetFloat("speed.x", rb.velocity.x);
        animator.SetFloat("speed.y", rb.velocity.y);
        if (rb.velocity.x < 0f)
        {
            spriteR.flipX = true;
        }
        else
        {
            spriteR.flipX = false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            attacking = 0.5f;
            animator.SetBool(name: "attacking", true);
            Debug.Log("well fuck you tooo");
        }
        
        if (attacking < 0f)
        {
            Debug.Log("nevermind");
            animator.SetBool(name: "attacking", false);
        }
        attacking -= Time.deltaTime;
        if (inventory.activeSelf && Input.GetMouseButton(0))
        {
            int tmpItem = whichItemToUse();
            if (tmpItem != -1)
            {
                switch (inventoryArray[tmpItem])
                {
                    case 1:
                        health += 1;
                        break;
                    case 2:
                        swords += 1;
                        break;
                    case 3:
                        stealth = 3f;
                        break;
                }
                inventoryArray[tmpItem] = 0;
                UpdateItems();
            }
        }
        //be super fuckin' sneak sneak
        GetComponent<SpriteRenderer>().enabled = (stealth < 0f);
        

            stealth -= Time.deltaTime;
    }

    int whichItemToUse()
    {
        float distance = 999f;
        int smallest = 0;
        for (int i = 0; i < slots.Length; i++)
        {
            float tmp = Vector2.Distance(slots[i].transform.position, Input.mousePosition);
            if (tmp < distance)
            {
                distance = tmp;
                smallest = i;
            }
        }

        if (distance > 2.6321f * (Screen.width / 100f)) return -1;
        //shootin' that hol' hors'
        // DIO's money more likely
        return smallest;
    }
    

    void OnTriggerStay2D(Collider2D other)
    {
        if (Input.GetButton("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpheight);
            Debug.Log("JUUUUUUUUUUUUUUUUUUUUMPAAAAH");
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "redpotion")
        {
            int tmp = findEmptySlot();
            if (tmp !=-1)
            {
                inventoryArray[tmp] = 1;
                Destroy(col.gameObject);
                UpdateItems();
            }
        }
        else if (col.gameObject.tag == "weapon")
        {
            int tmp = findEmptySlot();
            if (tmp !=-1)
            {
                inventoryArray[tmp] = 2;
                Destroy(col.gameObject);
                UpdateItems();
            }
        }
        else if (col.gameObject.tag == "bluepotion")
        {
            int tmp = findEmptySlot();
            if (tmp !=-1)
            {
                inventoryArray[tmp] = 3;
                Destroy(col.gameObject);
                UpdateItems();
            }
        }
    }

    void UpdateItems()
    {
        for (int i = 0; i < inventoryArray.Length; i++)
        {
            if (slots[i].transform.childCount == 1 && inventoryArray[i] == 0)
                Destroy(slots[i].transform.GetChild(0).gameObject);

            if (slots[i].transform.childCount == 0 && inventoryArray[i] != 0)
            {
                GameObject tmp = Instantiate(item, slots[i].transform.position, Quaternion.Euler(0,0,0));
                tmp.transform.parent = slots[i].transform;
                tmp.GetComponent<Image>().sprite = itemsprites[inventoryArray[i]-1];
            }
        }
    }
    int findEmptySlot ()
    {
        int firstSlot = -1;
        for(int i = 0; i < inventoryArray.Length;i++)
            if(firstSlot == -1 && inventoryArray[i] == 0) firstSlot = i;
        return firstSlot;
    }
}
