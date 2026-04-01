using UnityEngine;

public class CardCombinationCycler : MonoBehaviour
{
    [Header("References")]
    public PoppupDisplay popupDisplay;

    [Header("Cycle Settings")]
    public float cycleInterval = 1f;
    public bool autoCycle = true;

    private CardIngredient[] ingredients;
    private CardType[] types;

    private int ingredientIndex = 0;
    private int typeIndex = 0;

    private float timer;

    void Awake()
    {
        // Cache enum values
        ingredients = (CardIngredient[])System.Enum.GetValues(typeof(CardIngredient));
        types = (CardType[])System.Enum.GetValues(typeof(CardType));
    }

    void Start()
    {
        ApplyCurrent();
    }

    void Update()
    {
        if (!autoCycle) return;

        timer += Time.deltaTime;

        if (timer >= cycleInterval)
        {
            timer = 0f;
            NextCombination();
        }
    }

    public void NextCombination()
    {
        typeIndex++;

        if (typeIndex >= types.Length)
        {
            typeIndex = 0;
            ingredientIndex++;

            if (ingredientIndex >= ingredients.Length)
            {
                ingredientIndex = 0;
            }
        }

        ApplyCurrent();
    }

    public void PreviousCombination()
    {
        typeIndex--;

        if (typeIndex < 0)
        {
            typeIndex = types.Length - 1;
            ingredientIndex--;

            if (ingredientIndex < 0)
            {
                ingredientIndex = ingredients.Length - 1;
            }
        }

        ApplyCurrent();
    }

    void ApplyCurrent()
    {
        if (popupDisplay == null) return;

        CardData tempCard = new CardData
        {
            ingredient = ingredients[ingredientIndex],
            type = types[typeIndex]
        };

        popupDisplay.SetCard(tempCard);
    }
}