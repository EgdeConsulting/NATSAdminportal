import {
  AlertDialog,
  AlertDialogBody,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogContent,
  AlertDialogOverlay,
  Button,
} from "@chakra-ui/react";
import { SetStateAction, useRef } from "react";

function ActionConfirmation(props: {
  action: any;
  buttonText: string;
  alertHeader: string;
  isOpen: boolean;
  onClose: () => void;
  buttonDisable: boolean;
  toggleButtonDisable: any; //SetStateAction<boolean>;
}) {
  const cancelRef = useRef<any>();

  return (
    <AlertDialog
      isOpen={props.isOpen}
      leastDestructiveRef={cancelRef}
      onClose={props.onClose}
      isCentered={true}
    >
      <AlertDialogOverlay>
        <AlertDialogContent>
          <AlertDialogHeader fontSize="lg" fontWeight="bold">
            {props.alertHeader}
          </AlertDialogHeader>

          <AlertDialogBody>
            Are you sure? You can't undo this action afterwards.
          </AlertDialogBody>

          <AlertDialogFooter>
            <Button
              colorScheme="red"
              mr={3}
              onClick={() => {
                props.onClose();
                props.action();
                props.toggleButtonDisable(true);
              }}
            >
              {props.buttonText}
            </Button>
            <Button
              ref={cancelRef}
              onClick={() => {
                props.onClose();
              }}
            >
              Cancel
            </Button>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialogOverlay>
    </AlertDialog>
  );
}

export { ActionConfirmation };
