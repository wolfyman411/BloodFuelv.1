using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CardShuffler : MonoBehaviour
{
    public List<GameObject> availibleCards;
    public List<GameObject> unshuffledCards;
    public List<GameObject> shuffledCards;

    public GameObject upgradeCards;
    public GameObject[] card1;
    public GameObject[] card2;
    public GameObject[] card3;
    public GameObject[] card4;
    public GameObject[] card5;

    public float hoverHeight = 1f; // Desired height above the original position
    public float moveSpeed = 5f; // Speed of the object's movement
    private Vector3 originalPosition;
    private Vector3 targetPosition;
    public bool MoveCards;

    public int DisplayCards = 5;
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject card in availibleCards)
        {
            GameObject InstanceCard = card;
            unshuffledCards.Add(InstanceCard);
        }
        originalPosition = upgradeCards.transform.position;
        targetPosition = originalPosition;
        ShuffleCards("attack");
        changeCards();
    }

    public void changeCards()
    {
        DisplayUpdater(card1, shuffledCards[0]);
        DisplayUpdater(card2, shuffledCards[1]);
        DisplayUpdater(card3, shuffledCards[2]);
        DisplayUpdater(card4, shuffledCards[3]);
        DisplayUpdater(card5, shuffledCards[4]);
    }

    // Update is called once per frame
    void Update()
    {
        //Screensize Handler
        hoverHeight = (Screen.height) / 1.2f * -1;
        if (shuffledCards.Count <= 5)
        {
            ShuffleCards("");
        }

        //MovementHandler
        if (MoveCards)
        {
            targetPosition = originalPosition + Vector3.up * hoverHeight;
            StartCoroutine(MoveObject());
        }
        else
        {
            targetPosition = originalPosition;
            StartCoroutine(MoveObject());
        }

        //CardDisplay Handler
        if (DisplayCards == 5)
        {
            card5[3].gameObject.GetComponent<RectTransform>().transform.localScale = new Vector2(1, 1);
            card4[3].gameObject.GetComponent<RectTransform>().transform.localScale = new Vector2(1, 1);
        }
        else
        {
            card5[3].gameObject.GetComponent<RectTransform>().transform.localScale = new Vector2(0, 0);
            card4[3].gameObject.GetComponent<RectTransform>().transform.localScale = new Vector2(0,0);
        }
    }

    GameObject[] DisplayUpdater(GameObject[] currentCard, GameObject updateCard)
    {
        if (updateCard.GetComponent<BaseCardScript>())
        {
            BaseCardScript cardScript = updateCard.GetComponent<BaseCardScript>();
            //Title
            currentCard[0].GetComponent<Text>().text = cardScript.cardName;
            //Desc
            currentCard[2].GetComponent<Text>().text = cardScript.cardDesc;
            //Image
            currentCard[1].GetComponent<Image>().sprite = Sprite.Create(cardScript.cardImage, new Rect(0, 0, cardScript.cardImage.width, cardScript.cardImage.height), Vector2.one * 0.5f);
        }
        else
        {
            PassiveCardScript cardScript = updateCard.GetComponent<PassiveCardScript>();
            //Title
            currentCard[0].GetComponent<Text>().text = cardScript.cardName;
            //Desc
            currentCard[2].GetComponent<Text>().text = cardScript.cardDesc;
            //Image
            currentCard[1].GetComponent<Image>().sprite = Sprite.Create(cardScript.cardImage, new Rect(0, 0, cardScript.cardImage.width, cardScript.cardImage.height), Vector2.one * 0.5f);
        }
        return currentCard;
    }

    public void ShuffleCards(string type)
    {
        shuffledCards.Clear();
        while (unshuffledCards.Count > 0)
        {
            int posSelector = Random.Range(0, unshuffledCards.Count);
            shuffledCards.Add(unshuffledCards[posSelector]);
            unshuffledCards.RemoveAt(posSelector);
        }

        if (type == "attack")
        {
            List<GameObject> matchingCards = new List<GameObject>();

            //Locate attack cards and remove them
            for (int i = shuffledCards.Count - 1; i >= 0; i--)
            {
                GameObject card = shuffledCards[i];

                if (card.GetComponent<BaseCardScript>() && card.GetComponent<BaseCardScript>().cardType == "Attack")
                {
                    matchingCards.Add(card);
                    shuffledCards.RemoveAt(i);
                }
            }
            //Add them back to the list
            for (int i = matchingCards.Count - 1; i >= 0; i--)
            {
                shuffledCards.Insert(0, matchingCards[i]);
            }
        }
        if (type == "ability")
        {
            List<GameObject> matchingCards = new List<GameObject>();

            //Locate attack cards and remove them
            for (int i = shuffledCards.Count - 1; i >= 0; i--)
            {
                GameObject card = shuffledCards[i];

                if (card.GetComponent<BaseCardScript>() && card.GetComponent<BaseCardScript>().cardType == "Ability")
                {
                    matchingCards.Add(card);
                    shuffledCards.RemoveAt(i);
                }
            }
            //Add them back to the list
            for (int i = matchingCards.Count - 1; i >= 0; i--)
            {
                shuffledCards.Insert(0, matchingCards[i]);
            }
        }

        foreach (GameObject card in shuffledCards)
        {
            GameObject InstanceCard = card;
            unshuffledCards.Add(InstanceCard);
        }
    }

    private IEnumerator MoveObject()
    {
        while (upgradeCards.transform.position != targetPosition)
        {
            // Move the object towards the target position
            upgradeCards.transform.position = Vector3.MoveTowards(upgradeCards.transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
