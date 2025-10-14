using UnityEngine;

[System.Serializable]
public class Combo
{
   public InputReader.AttackInputResult[] Attacks;
   public InputReader.MovementInputResult[] Movement;
  // public InputReader.InputResults Results;
   public float timeLeft = 0.5f;
    
   public Combo() {}

   public Combo(ComboObj comboObj)
   {
      this.Attacks = comboObj.data.Attacks;
      this.Movement = comboObj.data.Movement;
      this.timeLeft = comboObj.data.timeLeft;
  //    this.Results = comboObj.data.Results;
   }
}

public abstract class ComboObj : ScriptableObject
{
   [Header("Combo Data")]
   public Combo data = new Combo();

   public Combo CreateCombo()
   {
      return new Combo(this);
   }
}
