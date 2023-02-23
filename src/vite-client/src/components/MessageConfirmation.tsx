import {
  AlertDialog,
  AlertDialogBody,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogContent,
  AlertDialogOverlay,
  Button,
  useDisclosure,
} from "@chakra-ui/react";
import { SetStateAction, useRef } from "react";

function MessageConfirmation(props: {
  publishMessage: any;
  buttonDisable: boolean;
  toggleButtonDisable: any; //SetStateAction<boolean>;
}) {
  const { isOpen, onOpen, onClose } = useDisclosure();
  const cancelRef = useRef<any>();

  return (
    <>
      <Button
        isDisabled={props.buttonDisable}
        colorScheme="blue"
        onClick={onOpen}
      >
        Publish
      </Button>

      <AlertDialog
        isOpen={isOpen}
        leastDestructiveRef={cancelRef}
        onClose={onClose}
        isCentered={true}
      >
        <AlertDialogOverlay>
          <AlertDialogContent>
            <AlertDialogHeader fontSize="lg" fontWeight="bold">
              Publish Message
            </AlertDialogHeader>

            <AlertDialogBody>
              Are you sure? You can't undo this action afterwards.
            </AlertDialogBody>

            <AlertDialogFooter>
              <Button
                colorScheme="red"
                mr={3}
                onClick={() => {
                  onClose();
                  props.publishMessage();
                  props.toggleButtonDisable(true);
                }}
              >
                Publish
              </Button>
              <Button
                ref={cancelRef}
                onClick={() => {
                  onClose();
                }}
              >
                Cancel
              </Button>
            </AlertDialogFooter>
          </AlertDialogContent>
        </AlertDialogOverlay>
      </AlertDialog>
    </>
  );
}

export { MessageConfirmation };
