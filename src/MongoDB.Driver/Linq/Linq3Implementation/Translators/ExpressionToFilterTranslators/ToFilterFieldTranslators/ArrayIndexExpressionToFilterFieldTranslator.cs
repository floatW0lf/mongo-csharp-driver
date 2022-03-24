﻿/* Copyright 2010-present MongoDB Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System.Linq.Expressions;
using MongoDB.Driver.Linq.Linq3Implementation.Ast.Filters;
using MongoDB.Driver.Linq.Linq3Implementation.ExtensionMethods;
using MongoDB.Driver.Linq.Linq3Implementation.Misc;

namespace MongoDB.Driver.Linq.Linq3Implementation.Translators.ExpressionToFilterTranslators.ToFilterFieldTranslators
{
    internal static class ArrayIndexExpressionToFilterFieldTranslator
    {
        public static AstFilterField Translate(TranslationContext context, BinaryExpression expression)
        {
            if (expression.NodeType == ExpressionType.ArrayIndex)
            {
                var arrayExpression = expression.Left;
                var arrayField = ExpressionToFilterFieldTranslator.Translate(context, arrayExpression);
                var indexExpression = expression.Right;
                var index = indexExpression.GetConstantValue<int>(containingExpression: expression);

                if (index < 0)
                {
                    var reason = "negative indexes are not valid";
                    if (index == -1)
                    {
                        reason += ". To use the positional operator $ use FirstMatchingElement instead of an index value of -1"; // closing period is added by exception
                    }
                    throw new ExpressionNotSupportedException(expression, because: reason);
                }

                var itemSerializer = ArraySerializerHelper.GetItemSerializer(arrayField.Serializer);
                return arrayField.SubField(index.ToString(), itemSerializer);
            }

            throw new ExpressionNotSupportedException(expression);
        }
    }
}
