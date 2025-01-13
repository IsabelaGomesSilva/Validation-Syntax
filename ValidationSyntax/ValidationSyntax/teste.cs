namespace ValidationSyntax
{
    public class teste
    {
        
    public Guid ProjectId { get; set; }
      public string Name { get; set; }
      public Guid i = Guid.NewGuid();  
    public bool Evaluate()      
    { 
     
      return (ProjectId == Guid.NewGuid());      }
        
    }
}