import {
  AlertDialog,
  AlertDialogBody,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogContent,
  AlertDialogOverlay,
  Button,
} from "@chakra-ui/react";
import { Dispatch, SetStateAction, useRef } from "react";

function ActionConfirmation(props: {
  action: any;
  isOpen: boolean;
  onClose: () => void;
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
            Action Confirmation
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
              }}
            >
              Confirm
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
