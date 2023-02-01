import {
  Table,
  Thead,
  Tbody,
  Tr,
  Th,
  Td,
  TableCaption,
  TableContainer,
} from "@chakra-ui/react";
import { ContentHider } from "./";

function MessageView(props: { messages: any[] }) {
  return (
    <TableContainer>
      <Table variant={"striped"} colorScheme={"gray"}>
        <TableCaption placement="top">All Messages</TableCaption>
        <Thead>
          <Tr>
            <Th>Timestamp</Th>
            <Th>Subject</Th>
            <Th>Acknowledgement</Th>
            <Th>Header</Th>
            <Th>Payload</Th>
          </Tr>
        </Thead>
        <Tbody>
          {props.messages.length != 0 &&
            props.messages.map((item, index) => {
              return (
                <Tr key={index} height={"75px"}>
                  <Td>{item.messageTimestamp}</Td>
                  <Td>{item.messageSubject}</Td>
                  <Td>{item.messageAck}</Td>
                  <Td whiteSpace={"normal"}>HEADER DATA HERE!</Td>
                  <Td whiteSpace={"normal"}>
                    <ContentHider content={item.messagePayload} />
                  </Td>
                </Tr>
              );
            })}
        </Tbody>
      </Table>
    </TableContainer>
  );
}

export { MessageView };
