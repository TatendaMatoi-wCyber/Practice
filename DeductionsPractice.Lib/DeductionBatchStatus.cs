using System;
using System.Collections.Generic;
using System.Text;

namespace DeductionsPractice.Lib
{
    public enum DeductionBatchStatus
    {
        DRAFT, 
        SAVED, 
        SENT, 
        PROCESSING, 
        PROCESSED, CANCELED, MERGED, AWAITING_APPROVAL, REJECTED, SKIPPED__DUPLICATES, AWAITING_DOWNLOAD, AWAITING_ATTACHMENTS
    }
}
