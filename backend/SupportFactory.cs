using System;
using System.Collections.Generic;
using System.Linq;

public static class SupportFactory
{
    public static List<SupportVector> GetSupportVectorList() => new()
    {
        new SupportVector { Description = "For billing-related inquiries, follow these steps: 1. Log into your account and navigate to the 'Billing' section. 2. Review your invoices and payment history. 3. Update your payment method if necessary. 4. If you have any discrepancies, contact our billing department at billing@xyzcompany.com." },
        new SupportVector { Description = "To view your bill, log into your account and navigate to the 'Billing' section. You can download or print your invoice from there." },
        new SupportVector { Description = "To update your payment method, go to the 'Billing' section of your account and select 'Payment Methods'. Follow the instructions to add or update your payment details." },
        new SupportVector { Description = "If you have a dispute with your bill, please contact our billing department at billing@xyzcompany.com with your invoice number and details of the issue." },
        new SupportVector { Description = "If you have forgotten your password or need to change it, follow these steps: 1. Go to the login page and click on 'Forgot Password'. 2. Enter your registered email address and submit. 3. Check your email for the password reset link and follow the instructions. 4. If you do not receive an email, check your spam folder or contact support." },
        new SupportVector { Description = "To renew your subscription, go to the 'Billing' section of your account and follow the instructions. If you encounter any issues, contact support." }
    };
}

public class SupportVector
{
    public string Description { get; set; } = string.Empty;
    public ReadOnlyMemory<float> Vector { get; set; }
}
