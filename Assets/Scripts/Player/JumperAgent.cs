using MLAgents;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class JumperAgent : Agent
{
    public float jumpForce;
    public float bounceForce;
    public float screenEdgeOffset;
    public float maxHorVel;

    public LevelGeneration LevelGen;
    public GameObject cam;
    public GameObject playerKiller;

    [Header("Perception")]
    public int numPerceivableObjs;
    public int observationRadius;
    

    private Rigidbody2D rb2d;
    private bool isMovingUp;
    private float maxHeight;

    public List<float> vectorObs;

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
       
    }

    public override void CollectObservations()
    {
        AddVectorObs((12f - (transform.position.x - playerKiller.transform.position.x)) / 12f);
        AddVectorObs((9.5f - (transform.position.y - playerKiller.transform.position.y)) / 9.5f);

        Collider2D[] temp = Physics2D.OverlapCircleAll(transform.position, observationRadius, 1 << 8);

        List<Collider2D> overlapObjs;

        overlapObjs = temp.OrderBy(x => Vector2.Distance(transform.position, x.transform.position)).ToList<Collider2D>();
        

        float j = 0;

        for (int i = 0; i < overlapObjs.Count; i++)
        {

            j++;

            if (i < numPerceivableObjs)
            {
                AddVectorObs(1);

                if (overlapObjs[i].gameObject.CompareTag("Enemy"))
                {
                    AddVectorObs(1);
                    AddVectorObs(0);
                    AddVectorObs(0);
                    AddVectorObs(0);
                }
                else if (overlapObjs[i].gameObject.CompareTag("Platform"))
                {
                    AddVectorObs(0);
                    AddVectorObs(1);
                    AddVectorObs(0);
                    AddVectorObs(0);
                }
                else if (overlapObjs[i].gameObject.CompareTag("BouncyPlatform"))
                {
                    AddVectorObs(0);
                    AddVectorObs(0);
                    AddVectorObs(1);
                    AddVectorObs(0);
                }
                else if (overlapObjs[i].gameObject.CompareTag("SlidingPlatform"))
                {
                    AddVectorObs(0);
                    AddVectorObs(0);
                    AddVectorObs(0);
                    AddVectorObs(1);
                }

                Vector2 positionNorm = (overlapObjs[i].transform.position - transform.position) / observationRadius;

                Vector2 distanceNorm = positionNorm;

                if (positionNorm.x >= 0)
                {
                    distanceNorm.x = 1 - positionNorm.x;
                }
                else
                {
                    distanceNorm.x = -1 - positionNorm.x;
                }

                if (positionNorm.y >= 0)
                {
                    distanceNorm.y = 1 - positionNorm.y;
                }
                else
                {
                    distanceNorm.y = -1 - positionNorm.y;
                }

                AddVectorObs(distanceNorm);


                Vector2 relSpeed = (Vector2.zero - rb2d.velocity) / (observationRadius * 2);

                if (overlapObjs[i].transform.position.x - transform.position.x > 0)
                {
                    relSpeed = new Vector2(-relSpeed.x, relSpeed.y);
                }

                if (overlapObjs[i].transform.position.y - transform.position.y > 0)
                {
                    relSpeed = new Vector2(relSpeed.x, -relSpeed.y);
                }

                AddVectorObs(relSpeed);

            }
            else
            {
                break;
            }
        }

        for (int i = 0; i < numPerceivableObjs - j; i++)
        {
            AddVectorObs(0);
            AddVectorObs(0);
            AddVectorObs(0);
            AddVectorObs(0);
            AddVectorObs(0);
            AddVectorObs(0);
            AddVectorObs(0);
            AddVectorObs(0);
            AddVectorObs(0);
        }

        vectorObs = info.vectorObservation;
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {

        var movement = (int)vectorAction[0];
        int direction = 0;

        switch (movement)
        {
            case 1:
                direction = -1;
                break;
            case 2:
                direction = 1;
                break;
        }


        rb2d.AddForce(new Vector2(direction * 100, 0));

        AddReward(Mathf.Abs(Mathf.Abs(direction) * -0.1f));


        //Continious

        //rb2d.AddForce(new Vector2(Mathf.Clamp(vectorAction[0], -1f, 1f) * 100, 0));

        //AddReward(Mathf.Abs(Mathf.Clamp(vectorAction[0], -1f, 1f)) * -0.1f);

        rb2d.velocity = new Vector2(Mathf.Clamp(rb2d.velocity.x, -maxHorVel, maxHorVel), rb2d.velocity.y);
    }

    private void FixedUpdate()
    {
        AddReward(0.01f);

        if (transform.position.y > maxHeight)
        {
            AddReward(rb2d.velocity.y / 30f);
            maxHeight = transform.position.y;
        }

        if (rb2d.velocity.y <= 0)
        {
            isMovingUp = false;
        }

        if (transform.position.x < transform.parent.position.x - screenEdgeOffset || transform.position.x > transform.parent.position.x + screenEdgeOffset)
        {
            transform.localPosition = new Vector3(-transform.localPosition.x, transform.position.y, transform.position.z);
        }

        if (transform.localPosition.y - 2 > cam.transform.localPosition.y)
        {
           cam.transform.localPosition = new Vector3(0, transform.position.y - 2, -30);
        }

    }

    public override void AgentReset()
    {
        transform.localPosition = Vector3.zero;
        rb2d.velocity = Vector2.zero;
        maxHeight = 0;
        isMovingUp = false;

        cam.transform.localPosition = new Vector3(0, 0, -30);

        LevelGen.ResetLevel();
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform") && !isMovingUp)
        {
            //AddReward(0.2f);
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpForce);
            isMovingUp = true;
        }
        else if (collision.gameObject.CompareTag("BouncyPlatform") && !isMovingUp)
        {
            //AddReward(0.5f);
            rb2d.velocity = new Vector2(rb2d.velocity.x, bounceForce);
            isMovingUp = true;
        }
        else if (collision.gameObject.CompareTag("SlidingPlatform") && !isMovingUp)
        {
            //AddReward(0.2f);
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpForce);
            isMovingUp = true;
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            AddReward(-1f);
            Done();
        }
    }
}
