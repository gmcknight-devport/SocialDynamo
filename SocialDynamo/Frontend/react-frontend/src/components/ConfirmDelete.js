import * as React from 'react';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogTitle from '@mui/material/DialogTitle';
import DeleteOutlineOutlinedIcon from '@mui/icons-material/DeleteOutlineOutlined';

export default function ConfirmDelete({handleConfirmDelete}) {
    const [open, setOpen] = React.useState(false);

    const handleClickOpen = () => {
        setOpen(true);
    };

    const handleClose = () => {
        setOpen(false);
    };

    const handleConfirm = () => {
        handleConfirmDelete();
        setOpen(false);
    };

    return (
        <React.Fragment>
        <DeleteOutlineOutlinedIcon onClick={handleClickOpen}/>
        <Dialog
            open={open}
            onClose={handleClose}
            aria-labelledby="alert-dialog-title"
        >
            <DialogTitle id="alert-dialog-title" sx={{backgroundColor: '#110f0f'}}>
                {"Are you sure you want to delete?"}
            </DialogTitle>
            <DialogActions sx={{backgroundColor: '#110f0f', justifyContent: 'center'}}>
                <Button className='confirm-close' onClick={handleConfirm} sx={{border: '1px solid', borderRadius: '5px'}}>
                    Yes
                </Button>
                <Button className='refuse-close' onClick={handleClose} sx={{border: '1px solid', borderRadius: '5px'}}>
                    No
                </Button>                
            </DialogActions>
        </Dialog>
        </React.Fragment>
    );
}