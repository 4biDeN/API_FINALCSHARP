using FluentValidation;
using apifinal.Services.DTOs;
using System.Text.RegularExpressions;
using System;

namespace apifinal.Services.Validate
{
    public class ProductBaseValidator<T> : AbstractValidator<T> where T : ProductBaseDTO
    {
        public ProductBaseValidator()
        {
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("O Campo Description é obrigatório.");

            RuleFor(x => x.Barcode)
                .NotEmpty().WithMessage("O Campo Barcode é obrigatório.")
                .Must(IsValidBarcode).WithMessage("O Barcode fornecido não corresponde ao tipo especificado.");

            RuleFor(x => x.Barcodetype)
                .NotEmpty().WithMessage("O Campo Barcodetype é obrigatório.")
                .Must(IsValidBarcodeType).WithMessage("Tipo de código de barras inválido.");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("O Campo Price não pode ser negativo.");

            RuleFor(x => x.Costprice)
                .GreaterThanOrEqualTo(0).WithMessage("O Campo Costprice não pode ser negativo.");
        }

        private bool IsValidBarcodeType(string barcodetype)
        {
            string[] validBarcodeTypes = { "EAN-13", "DUN-14", "UPC", "CODE 11", "CODE 39" };
            return Array.IndexOf(validBarcodeTypes, barcodetype) != -1;
        }

        private bool IsValidBarcode(ProductBaseDTO product, string barcode)
        {
            switch (product.Barcodetype)
            {
                case "EAN-13":
                    return IsEAN13(barcode);
                case "DUN-14":
                    return IsDUN14(barcode);
                case "UPC":
                    return IsUPC(barcode);
                case "CODE 11":
                    return IsCODE11(barcode);
                case "CODE 39":
                    return IsCODE39(barcode);
                default:
                    return false;
            }
        }
        private static bool IsEAN13(string barcode)
        {
            if (!Regex.IsMatch(barcode, @"^\d{13}$"))
            {
                return false;
            }

            int sumOdd = 0;
            int sumEven = 0;
            for (int i = 0; i < 12; i++)
            {
                int digit = int.Parse(barcode[i].ToString());
                if (i % 2 == 0)
                {
                    sumOdd += digit;
                }
                else
                {
                    sumEven += digit;
                }
            }

            int total = sumOdd + (sumEven * 3);
            int remainder = total % 10;
            int checksum = remainder == 0 ? 0 : 10 - remainder;

            return checksum == int.Parse(barcode[12].ToString());
        }

        private static bool IsDUN14(string barcode)
        {
            if (!Regex.IsMatch(barcode, @"^\d{14}$"))
            {
                return false;
            }

            char firstDigit = barcode[0];
            if (firstDigit != '1' && firstDigit != '2' && firstDigit != '3' && firstDigit != '4' && firstDigit != '5')
            {
                return false;
            }

            string productCode = barcode.Substring(1, 12);
            return IsEAN13(productCode);
        }

        private static bool IsUPC(string barcode)
        {
            if (!Regex.IsMatch(barcode, @"^\d{12}$"))
            {
                return false;
            }

            char firstDigit = barcode[0];
            if (firstDigit != '0' && firstDigit != '2' && firstDigit != '3' && firstDigit != '4' && firstDigit != '5' && firstDigit != '6' && firstDigit != '7')
            {
                return false;
            }

            int sumOdd = 0;
            int sumEven = 0;
            for (int i = 0; i < 11; i++)
            {
                int digit = int.Parse(barcode[i].ToString());
                if (i % 2 == 0)
                {
                    sumOdd += digit;
                }
                else
                {
                    sumEven += digit;
                }
            }

            int total = (sumEven * 3) + sumOdd;
            int checksum = total % 10;
            checksum = (10 - checksum) % 10;

            return checksum == int.Parse(barcode[11].ToString());
        }

        private static bool IsCODE11(string barcode)
        {
            return Regex.IsMatch(barcode, @"^[0-9\*\-]+$");
        }

        private static bool IsCODE39(string barcode)
        {
            return Regex.IsMatch(barcode, @"^\*[A-Za-z0-9\-\.\/\+\%\s]+\*$");
        }
    }
}
