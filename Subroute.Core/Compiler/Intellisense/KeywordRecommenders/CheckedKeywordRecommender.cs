// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis.CSharp;

namespace Subroute.Core.Compiler.Intellisense.KeywordRecommenders
{
    internal class CheckedKeywordRecommender : AbstractSyntacticSingleKeywordRecommender
    {
        public CheckedKeywordRecommender()
            : base(SyntaxKind.CheckedKeyword)
        {
        }

        protected override bool IsValidContext(int position, CSharpSyntaxContext context, CancellationToken cancellationToken)
        {
            return
                context.IsStatementContext ||
                context.IsGlobalStatementContext ||
                context.IsNonAttributeExpressionContext;
        }
    }
}
