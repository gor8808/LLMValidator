namespace LLMValidation.Prompts;

public sealed class StaticPromptAdapter<T> : IStaticPromptAdapter
    where T : IPromptTemplate
{
    public string Get(PromptVariant q)
    {   
        return q switch
        {
            PromptVariant.Fast => T.FastPrompt,
            PromptVariant.Balanced => T.BalancedPrompt,
            PromptVariant.Accurate => T.AccuratePrompt,
            _ => throw new ArgumentOutOfRangeException(nameof(q))
        };
    }
}

public sealed class StaticPromptAdapter<T, TArgs> : IStaticPromptAdapter
    where T : IPromptTemplate<TArgs>
{
    public string Get(PromptVariant q)
    {   
        return q switch
        {
            PromptVariant.Fast => T.FastPrompt,
            PromptVariant.Balanced => T.BalancedPrompt,
            PromptVariant.Accurate => T.AccuratePrompt,
            _ => throw new ArgumentOutOfRangeException(nameof(q))
        };
    }
}

public interface IStaticPromptAdapter
{
    public string Get(PromptVariant q);
}