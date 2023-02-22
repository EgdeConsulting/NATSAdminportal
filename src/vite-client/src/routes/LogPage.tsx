import { VStack } from "@chakra-ui/react";
import { PaginatedTable } from "components";

function LogPage() {
  const columns = [
    {
      Header: "All Events",
      columns: [
        {
          Header: "Timestamp",
          accessor: "timestamp",
          appendChildren: "false",
        },
        {
          Header: "Subject",
          accessor: "subject",
          appendChildren: "false",
        },
        {
          Header: "Acknowledgement",
          accessor: "acknowledgement",
          appendChildren: "false",
        },
        {
          Header: "Headers",
          accessor: "headers",
          appendChildren: "true",
        },
        {
          Header: "Payload",
          accessor: "payload",
          appendChildren: "true",
        },
      ],
    },
  ];

  return <VStack align="stretch" margin={2} w="80%" h="100%"></VStack>;
}

export { LogPage };
