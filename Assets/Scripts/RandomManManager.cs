using UnityEditor.UI;
using UnityEngine;

public class RandomManManager : MonoBehaviour
{
    private Material[] SkinMaterials = { };
    private Material[] ClothMaterials = { };
    private Material TestMat = null;

    private MeshRenderer HatRenderer = null;
    private MeshRenderer[] SkinRenderers = { };
    private MeshRenderer[] TShirtRenderers = { };
    private MeshRenderer[] BottomsRenderers = { };

    [SerializeField] private string PrefabFellaCode = null;
    [SerializeField] private string PrefabFellaCurrentCode = null;

    private void Awake()
    {
        SkinMaterials = Resources.LoadAll<Material>("Skin");
        ClothMaterials = Resources.LoadAll<Material>("Cloth");
        TestMat = Resources.LoadAll<Material>("Test")[0];

        GetRenderers();
        ApplyMaterials();
    }

    private void FixedUpdate()
    {
        //ChangeTestMatColour();
        if (PrefabFellaCode.Length == 4)
        {
            PrefabFellasSetColour(PrefabFellaCode);
        }
        PrefabFellaCurrentCode = PrefabFellasGetCode();
    }

    private void GetRenderers()
    {
        HatRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        SkinRenderers = transform.GetChild(1).GetComponentsInChildren<MeshRenderer>();
        TShirtRenderers = transform.GetChild(2).GetComponentsInChildren<MeshRenderer>();
        BottomsRenderers = transform.GetChild(3).GetComponentsInChildren<MeshRenderer>();
    }

    private void ApplyMaterials()
    {
        HatRenderer.material = PickACloth();
        
        Material PickedSkin = PickASkin();
        for (int i = 0; i < SkinRenderers.Length; i++)
        {
            SkinRenderers[i].material = PickedSkin;
        }

        Material PickedCloth = PickACloth();
        for (int i = 0; i < TShirtRenderers.Length; i++)
        {
            TShirtRenderers[i].material = PickedCloth;
        }
        PickedCloth = PickACloth();
        for (int i = 0; i < BottomsRenderers.Length; i++)
        {
            BottomsRenderers[i].material = PickedCloth;
        }
    }

    private Material PickASkin()
    {
        int RandomSkinNumber = Random.Range(0, SkinMaterials.Length);
        return SkinMaterials[RandomSkinNumber];
    }

    private Material PickACloth()
    {
        int RandomClothNumber = Random.Range(0, ClothMaterials.Length);
        return ClothMaterials[RandomClothNumber];
    }

    private void ChangeTestMatColour()
    {
        TestMat.color = Random.ColorHSV();
    }

    private void PrefabFellasSetColour(string FourDigitColours)
    {
        //Colour digits meaning guide.
        //-Digit 1 - Hat Colour.
        //-Digit 2 - Skin Colour.
        //-Digit 3 - TShirt Colour.
        //-Digit 4 - Bottoms Colour.
        //-Digits 1, 3, & 4 use the materials in Resources/Cloth.
        //-Digit 2 uses the materials in Resources/Skin.
        //-Number is the position of desired colour in its folder, starting at 0. Order is automatically set to alphabetical.
        //.

        char HatNum = FourDigitColours[0];
        char SkinNum = FourDigitColours[1];
        char TShirtNum = FourDigitColours[2];
        char BottomsNum = FourDigitColours[3];

        HatRenderer.material = ClothMaterials[(int)HatNum - 48]; //The 48 is converting ASCII and normal person numbers.
        
        for (int i = 0; i < SkinRenderers.Length; i++)
        {
            SkinRenderers[i].material = SkinMaterials[(int)SkinNum - 48];
        }
        
        for (int i = 0; i < TShirtRenderers.Length; i++)
        {
            TShirtRenderers[i].material = ClothMaterials[(int)TShirtNum - 48];
        }
        
        for (int i = 0; i < BottomsRenderers.Length; i++)
        {
            BottomsRenderers[i].material = ClothMaterials[(int)BottomsNum - 48];
        }
    }

    private string PrefabFellasGetCode()
    {
        int HatNum = GetClothColourNumber(HatRenderer.sharedMaterial) + 48; //+48 switches from normal numbers to their ASCII values.
        int SkinNum = GetSkinColourNumber(SkinRenderers[0].sharedMaterial) + 48;
        int TShirtNum = GetClothColourNumber(TShirtRenderers[0].sharedMaterial) + 48;
        int BottomsNum = GetClothColourNumber(BottomsRenderers[0].sharedMaterial)+ 48;

        string FellaCode = new string(new char[4] {(char)HatNum, (char)SkinNum, (char)TShirtNum, (char)BottomsNum});
        return FellaCode;
    }

    private int GetSkinColourNumber(Material ColourToNumber)
    {
        for (int i = 0; i < SkinMaterials.Length; i++)
        {
            if (SkinMaterials[i] == ColourToNumber)
            {
                return i;
            }
        }
        return -1;
    }
    private int GetClothColourNumber(Material ColourToNumber)
    {
        for (int i = 0; i < ClothMaterials.Length; i++)
        {
            if (ClothMaterials[i] == ColourToNumber)
            {
                return i;
            }
        }
        return -1;
    }
}
