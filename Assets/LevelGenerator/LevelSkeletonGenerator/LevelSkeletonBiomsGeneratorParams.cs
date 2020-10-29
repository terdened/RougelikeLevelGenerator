public class LevelSkeletonBiomsGeneratorParams
{
    public double MaxOpenSpacePerimeter { get; set; }
    public double MinOpenSpacePerimeter { get; set; }
    public LevelSkeleton LevelSkeleton { get; set; }

    public LevelSkeletonBiomsGeneratorParams()
    {
        MaxOpenSpacePerimeter = 450;
        MinOpenSpacePerimeter = 80;
    }
}
