using UnityEngine;

public interface IAgentController {
   void moveTo(Vector3 to);
   void moveTo(GameObject to);
   void runTo(Vector3 to);
   void drink();
   void eat();
   void sleep();
   void seeAround();
   void ResetCoroutines();
   void BeginSolvingState();
   bool IsSolving();
   bool IsGoing();
   void Going();
   void IsThere();
   bool IsHere(Vector3 to);
   float SizeWithAge();
   Species GetSpecies();
   double GetAge();
   GameObject getBestWaterPosition(); 
   Agent getBestFoodPosition();
   Agent findMate();
}
