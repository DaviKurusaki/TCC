using UnityEngine;

public class PlayerMagicSelector : MonoBehaviour
{
    public enum MagicType { Normal, Fire, Ice, Lightning }
    public MagicType currentMagic = MagicType.Normal;

    public void SetMagic(string type)
    {
        switch (type)
        {
            case "fire": currentMagic = MagicType.Fire; break;
            case "ice": currentMagic = MagicType.Ice; break;
            case "lightning": currentMagic = MagicType.Lightning; break;
        }
    }

    public MagicType GetMagicType()
    {
        return currentMagic;
    }
}
