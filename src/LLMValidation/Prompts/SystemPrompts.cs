namespace LLMValidation.Prompts;

/// <summary>
/// Contains all system prompts used by the LLM validator.
/// </summary>
public static class SystemPrompts
{
    public class Default : IPromptTemplate
    {
        public static string FastPrompt => """
                                           You are a text validator. Respond with JSON only: {"is_valid": boolean, "reason": string|null}
                                           Rules: is_valid=true if text meets ALL criteria, reason only when invalid, be fast.
                                           """;
        public static string BalancedPrompt => """
                                               You are a text validator. Evaluate text against criteria and respond with JSON only:
                                               {"is_valid": boolean, "reason": string|null}

                                               Rules:
                                               - is_valid: true if text meets ALL criteria, false otherwise
                                               - reason: brief explanation only when invalid (null when valid)
                                               - Be decisive and fast
                                               """;
        public static string AccuratePrompt => """
                                               You are a specialized text validation assistant. Your role is to evaluate text against specific validation criteria and return structured results.

                                               ## Response Format:
                                               Respond with valid JSON only, using this exact structure:
                                               {
                                                 "is_valid": boolean,
                                                 "reason": string or null
                                               }

                                               ## Guidelines:
                                               - Set "is_valid" to true if text meets ALL validation criteria
                                               - Set "is_valid" to false if text fails ANY validation criteria
                                               - Include "reason" only when is_valid is false (set to null when valid)
                                               - Keep reasons under 100 characters and factual
                                               - Focus on the most critical validation failure
                                               - Be decisive - avoid ambiguous language
                                               - Prioritize accuracy over speed
                                               """;
    }
    
    public class Meaning : IPromptTemplate<Meaning.Args>
    {
        public class Args
        {
            
        }

        public static string FastPrompt { get; }
        public static string BalancedPrompt { get; }
        public static string AccuratePrompt { get; }
        
        public Args Arguments { get; set; }
    }
    
    
}