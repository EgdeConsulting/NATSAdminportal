import { PaginatedTable } from "./";

function MessageView(props: { messages: any[] }) {
  const columns = [
    {
      Header: "All Messages",
      columns: [
        {
          Header: "Timestamp",
          accessor: "messageTimestamp",
          hideContent: "false",
        },
        {
          Header: "Subject",
          accessor: "messageSubject",
          hideContent: "false",
        },
        {
          Header: "Acknowledgement",
          accessor: "messageAcknowledgement",
          hideContent: "false",
        },
        {
          Header: "Headers",
          accessor: "messageHeaders",
          hideContent: "true",
        },
        {
          Header: "Payload",
          accessor: "messagePayload",
          hideContent: "true",
        },
      ],
    },
  ];

  return <PaginatedTable columns={columns} data={props.messages} />;
}

export { MessageView };
